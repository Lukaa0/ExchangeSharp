namespace ExchangeSharp.API.Exchanges.Poloniex
{
	public class GetOrderHistoryRequestModel
	{
		public string Symbol { get; set; }
		public string PoloSide { get; set; }
		public string Type { get; set; }
		public string AccountType { get; set; }
		public double? Price { get; set; }
		public int? Quantity { get; set; }
		public double? Amount { get; set; }
		public string Direction { get; set; }
		public long? From { get; set; }
		public string States { get; set; }
		public long? StartTime { get; set; }
		public long? EndTime { get; set; }
		public int? Limit { get; set; }
		public bool HideCancel { get; set; }
	}

}
