using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class AuthenticationModel
	{
		[JsonProperty("key")]
		public string Key { get; set; }
		[JsonProperty("signTimestamp")]
		public string SignTimestamp { get; set; }
		[JsonProperty("signatureMethod")]
		public string SignatureMethod { get; set; }
		[JsonProperty("signatureVersion")]
		public string SignatureVersion { get; set; }
		[JsonProperty("signature")]
		public string Signature { get; set; }

		public AuthenticationModel(string key, string signTimestamp, string signature)
		{
			Key = key;
			SignTimestamp = signTimestamp;
			Signature = signature;
			SignatureMethod = "HmacSHA256";
			SignatureVersion = "2";
		}
	}
}
