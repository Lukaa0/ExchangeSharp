using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class PoloniexBook
	{
		[JsonProperty("symbol")]
		public string Symbol { get; set; }
		[JsonProperty("createTime")]
		public long CreateTime { get; set; }
		[JsonProperty("bids")]
		public List<List<string>> Bids { get; set; }
		[JsonProperty("asks")]
		public List<List<string>> Asks { get; set; }
		[JsonProperty("ts")]
		public long Timestamp { get; set; }
		[JsonProperty("id")]
		public long Id { get; set; }
	}
}
