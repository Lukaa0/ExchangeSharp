namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class GetOpenOrdersRequestModel
	{
		public string Symbol { get; set; }
		public string PoloSide { get; set; }
		public long? From { get; set; }
		public string Direction { get; set; }
		public int? Limit { get; set; }
	}
}
