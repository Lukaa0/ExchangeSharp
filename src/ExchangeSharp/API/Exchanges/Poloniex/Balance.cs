using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public sealed class Balance
	{
		[JsonProperty("currencyId")]
		public string CurrencyId { get; set; }
		[JsonProperty("currency")]
		public string Currency { get; set; }
		[JsonProperty("available")]
		public string Available { get; set; }
		[JsonProperty("hold")]
		public string Hold { get; set; }
	}
}
