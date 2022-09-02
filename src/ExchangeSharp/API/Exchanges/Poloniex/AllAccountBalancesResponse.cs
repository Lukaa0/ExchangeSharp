using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public sealed class AllAccountBalancesResponse
	{
		[JsonProperty("accountId")]
		public string AccountId { get; set; }
		[JsonProperty("accountType")]
		public string AccountType { get; set; }
		[JsonProperty("balances")]
		public List<Balance> Balances { get; set; }
	}
}
