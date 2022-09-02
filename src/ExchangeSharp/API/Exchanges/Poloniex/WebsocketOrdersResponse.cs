using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class WebSocketOrderResponse
	{
		[JsonProperty("symbol")]
		public string Symbol { get; set; }
		[JsonProperty("type")]
		public string Type { get; set; }
		[JsonProperty("quantity")]
		public string Quantity { get; set; }
		[JsonProperty("orderId")]
		public string OrderId { get; set; }
		[JsonProperty("tradeFee")]
		public string TradeFee { get; set; }
		[JsonProperty("clientOrderId")]
		public string ClientOrderId { get; set; }
		[JsonProperty("accountType")]
		public string AccountType { get; set; }
		[JsonProperty("feeCurrency")]
		public string FeeCurrency { get; set; }
		[JsonProperty("eventType")]
		public string EventType { get; set; }
		[JsonProperty("source")]
		public string Source { get; set; }
		[JsonProperty("side")]
		public string Side { get; set; }
		[JsonProperty("filledQuantity")]
		public string FilledQuantity { get; set; }
		[JsonProperty("filledAmount")]
		public string FilledAmount { get; set; }
		[JsonProperty("matchRole")]
		public string MatchRole { get; set; }
		[JsonProperty("state")]
		public string State { get; set; }
		[JsonProperty("tradeTime")]
		public int TradeTime { get; set; }
		[JsonProperty("tradeAmount")]
		public string TradeAmount { get; set; }
		[JsonProperty("orderAmount")]
		public string OrderAmount { get; set; }
		[JsonProperty("createTime")]
		public long CreateTime { get; set; }
		[JsonProperty("price")]
		public string Price { get; set; }
		[JsonProperty("tradeQty")]
		public string TradeQty { get; set; }
		[JsonProperty("tradePrice")]
		public string TradePrice { get; set; }
		[JsonProperty("tradeId")]
		public string TradeId { get; set; }
		[JsonProperty("ts")]
		public long Timestamp { get; set; }
	}
}
