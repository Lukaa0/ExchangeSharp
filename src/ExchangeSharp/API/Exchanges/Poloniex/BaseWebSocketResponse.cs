using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class BaseWebSocketResponse<WebSocketData>
	{
		[JsonProperty("data")]
		public WebSocketData Data { get; set; }
		[JsonProperty("channel")]
		public string Channel { get; set; }
	}
}
