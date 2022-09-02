using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	internal sealed class PoloniexBookResponse
	{
		[JsonProperty("channel")]
		public string Channel { get; set; }
		[JsonProperty("data")]
		public List<PoloniexBook> Data { get; set; }
	}
}
