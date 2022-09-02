using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class WebSocketAuthenticationResponse
	{
		[JsonProperty("success")]
		public bool Success { get; set; }
		[JsonProperty("ts")]
		public long Timestamp { get; set; }
	}
}
